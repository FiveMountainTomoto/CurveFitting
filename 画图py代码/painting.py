import matplotlib.pyplot as plt

def read_points_from_file(filename):
    with open(filename, 'r') as file:
        # 根据文件内容，这里有两种不同的读取方式
        if '测试数据' in filename:
            points = [tuple(map(float, line.strip().split(',')[1:3])) for line in file if line.strip()]
        else:
            points = [tuple(map(float, line.strip().split(','))) for line in file if line.strip()]
    return points


def plot_points(points, connect=False, marker='o', label='Data Points', color='red'):
    x_values, y_values = zip(*points)

    plt.scatter(x_values, y_values, color=color, label=label, marker=marker)

    if connect:
        plt.plot(x_values, y_values, color='black', label='Connected Line')

    plt.xlabel('X-axis')
    plt.ylabel('Y-axis')
    plt.grid(True)


# 绘制测试数据点
test_data_filename = '测试数据.txt'


# 绘制闭合曲线拟合点坐标，连接点成线
closed_curve_filename = '闭合曲线拟合点坐标.txt'
plot_points(read_points_from_file(
    "C:\\Users\\86159\\source\\repos\\第49章 利用五点光滑法进行曲线拟合\\计算结果\\闭合曲线拟合点坐标.txt"),
    connect=True,color='white')
plot_points(
    read_points_from_file("C:\\Users\\86159\\source\\repos\\第49章 利用五点光滑法进行曲线拟合\\数据\\测试数据.txt"),
    color='blue')
plt.show()
# 绘制不闭合曲线拟合点坐标，连接点成线
open_curve_filename = '不闭合曲线拟合点坐标.txt'
plot_points(read_points_from_file(
    "C:\\Users\\86159\\source\\repos\\第49章 利用五点光滑法进行曲线拟合\\计算结果\\不闭合曲线拟合点坐标.txt"),
    connect=True,color='white')
plot_points(
    read_points_from_file("C:\\Users\\86159\\source\\repos\\第49章 利用五点光滑法进行曲线拟合\\数据\\测试数据.txt"),
    color='blue')
plt.show()
